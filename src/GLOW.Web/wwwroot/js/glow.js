/* ==========================================================================
   GLOW — Core JavaScript & Hardware APIs (Camera, Geolocation, Timer)
   ========================================================================== */

document.addEventListener('DOMContentLoaded', () => {
    // 1. Tarot Card Flip Trigger
    document.querySelectorAll('.tarot-card').forEach(card => {
        card.addEventListener('click', () => {
            card.classList.toggle('flipped');
        });
    });

    // 2. Interactive Study/Work Pomodoro Timer
    let timerInterval = null;
    let secondsLeft = 25 * 60;
    const timerDisplay = document.getElementById('timer-display');
    const startBtn = document.getElementById('timer-start');
    const pauseBtn = document.getElementById('timer-pause');
    const resetBtn = document.getElementById('timer-reset');

    function updateTimerDisplay() {
        if (!timerDisplay) return;
        const mins = Math.floor(secondsLeft / 60);
        const secs = secondsLeft % 60;
        timerDisplay.textContent = `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`;
    }

    if (startBtn) {
        startBtn.addEventListener('click', () => {
            if (timerInterval) return;
            timerInterval = setInterval(() => {
                if (secondsLeft > 0) {
                    secondsLeft--;
                    updateTimerDisplay();
                } else {
                    clearInterval(timerInterval);
                    timerInterval = null;
                    alert('🎉 Hoàn thành phiên tập trung! Hãy nghỉ ngơi 5 phút nha!');
                }
            }, 1000);
        });
    }

    if (pauseBtn) {
        pauseBtn.addEventListener('click', () => {
            clearInterval(timerInterval);
            timerInterval = null;
        });
    }

    if (resetBtn) {
        resetBtn.addEventListener('click', () => {
            clearInterval(timerInterval);
            timerInterval = null;
            secondsLeft = 25 * 60;
            updateTimerDisplay();
        });
    }
});

// 3. Camera API Helper
let videoStream = null;

async function startCamera(videoElementId) {
    const video = document.getElementById(videoElementId);
    if (!video) return;

    try {
        videoStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: false });
        video.srcObject = videoStream;
        video.play();
    } catch (err) {
        alert('Không thể mở Camera: ' + err.message);
    }
}

function stopCamera() {
    if (videoStream) {
        videoStream.getTracks().forEach(track => track.stop());
        videoStream = null;
    }
}

function captureSnapshot(videoElementId, canvasElementId, inputTargetId) {
    const video = document.getElementById(videoElementId);
    const canvas = document.getElementById(canvasElementId);
    const input = document.getElementById(inputTargetId);

    if (!video || !canvas) return;

    const context = canvas.getContext('2d');
    canvas.width = video.videoWidth || 640;
    canvas.height = video.videoHeight || 480;
    context.drawImage(video, 0, 0, canvas.width, canvas.height);

    const dataUrl = canvas.toDataURL('image/jpeg');
    if (input) {
        input.value = dataUrl;
    }
    stopCamera();
    alert('📸 Đã chụp ảnh thành công!');
}

// 4. Geolocation API Helper
function getCurrentLocation(latInputId, lngInputId, statusElemId) {
    const status = document.getElementById(statusElemId);
    if (!navigator.geolocation) {
        if (status) status.textContent = 'Trình duyệt không hỗ trợ định vị GPS.';
        return;
    }

    if (status) status.textContent = 'Đang lấy tọa độ GPS... 📍';

    navigator.geolocation.getCurrentPosition(
        (position) => {
            const lat = position.coords.latitude;
            const lng = position.coords.longitude;
            const latInput = document.getElementById(latInputId);
            const lngInput = document.getElementById(lngInputId);

            if (latInput) latInput.value = lat.toFixed(5);
            if (lngInput) lngInput.value = lng.toFixed(5);
            if (status) status.textContent = `📍 Đã nhận tọa độ: ${lat.toFixed(4)}, ${lng.toFixed(4)}`;
        },
        (error) => {
            if (status) status.textContent = 'Không thể lấy vị trí: ' + error.message;
        }
    );
}
