import cv2
import mediapipe as mp
import math
import socket
import threading

mp_face_mesh = mp.solutions.face_mesh
mp_drawing = mp.solutions.drawing_utils

# نقاط العين
LEFT_EYE = [33, 160, 158, 133, 153, 144]
RIGHT_EYE = [362, 385, 387, 263, 373, 380]

def euclidean_distance(p1, p2):
    return math.dist(p1, p2)

def eye_aspect_ratio(landmarks, eye_points, image_w, image_h):
    coords = [(int(landmarks[p].x * image_w), int(landmarks[p].y * image_h)) for p in eye_points]
    vertical1 = euclidean_distance(coords[1], coords[5])
    vertical2 = euclidean_distance(coords[2], coords[4])
    horizontal = euclidean_distance(coords[0], coords[3])
    EAR = (vertical1 + vertical2) / (2.0 * horizontal)
    return EAR

# ---------------- SOCKET SERVER ----------------
clients = []

def client_handler(conn, addr):
    print(f"[NEW CLIENT] {addr} connected")
    clients.append(conn)
    try:
        while True:
            data = conn.recv(1024)  # ننتظر أي رسالة من الكلاينت (اختياري)
            if not data:
                break
    except:
        pass
    finally:
        print(f"[DISCONNECT] {addr}")
        clients.remove(conn)
        conn.close()

def start_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("127.0.0.1", 9999))
    server.listen()
    print("[SERVER] Listening on 127.0.0.1:9999")
    while True:
        conn, addr = server.accept()
        thread = threading.Thread(target=client_handler, args=(conn, addr))
        thread.start()

# نشغل السيرفر في Thread منفصل
server_thread = threading.Thread(target=start_server, daemon=True)
server_thread.start()

# ---------------- BLINK DETECTION ----------------
cap = cv2.VideoCapture(0)
with mp_face_mesh.FaceMesh(
    max_num_faces=1,
    refine_landmarks=True,
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5
) as face_mesh:

    BLINK_THRESHOLD = 0.25
    blink_sent = False  # عشان ما يرسل Blink أكثر من مرة في نفس الرمش

    while cap.isOpened():
        ret, frame = cap.read()
        if not ret:
            break

        h, w, _ = frame.shape
        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = face_mesh.process(rgb_frame)

        if results.multi_face_landmarks:
            for face_landmarks in results.multi_face_landmarks:
                left_ear = eye_aspect_ratio(face_landmarks.landmark, LEFT_EYE, w, h)
                right_ear = eye_aspect_ratio(face_landmarks.landmark, RIGHT_EYE, w, h)
                ear = (left_ear + right_ear) / 2.0

                mp_drawing.draw_landmarks(frame, face_landmarks, mp_face_mesh.FACEMESH_CONTOURS)

                if ear < BLINK_THRESHOLD and not blink_sent:
                    cv2.putText(frame, "Blink!", (50, 100),
                                cv2.FONT_HERSHEY_SIMPLEX, 2, (0, 0, 255), 3)
                    # إرسال لجميع الكلاينت
                    for c in clients:
                        try:
                            c.sendall(b"Blink\n")
                        except:
                            pass
                    blink_sent = True
                elif ear >= BLINK_THRESHOLD:
                    blink_sent = False

        cv2.imshow("Blink Detection", frame)
        if cv2.waitKey(1) & 0xFF == 27:  # ESC
            break

cap.release()
cv2.destroyAllWindows()
