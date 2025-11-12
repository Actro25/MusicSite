const playerButtonPlay = document.getElementById("play-track-button");
const playerButtonNextTrack = document.getElementById("play-next-track-button");
const playerButtonPreviousTrack = document.getElementById("play-previous-track-button");
const playerInputTrackProgress = document.getElementById("play-track-input");
const playerInputVolumeProgres = document.getElementById("volume-track-input");
const spanTextProgress = document.getElementById("progress-lenght-track");
const spanTextAudioLenght = document.getElementById("end-lenght-track");
const audio = document.getElementById("track-player");
let isPlay = false;
document.addEventListener('DOMContentLoaded', function () {
    playerButtonPlay.disabled = true;
    playerInputTrackProgress.disabled = true;
    playerInputVolumeProgres.disabled = true;
    playerButtonNextTrack.disabled = true;
    playerButtonPreviousTrack.disabled = true;

    playerButtonPlay.style.opacity = '0.5';
    playerInputTrackProgress.style.opacity = '0.5';
    playerInputVolumeProgres.style.opacity = '0.5';
    playerButtonNextTrack.style.opacity = '0.5';
    playerButtonPreviousTrack.style.opacity = '0.5';
});
window.addEventListener('BottomSidePanelReceive', (e) => {
    if (e.detail.audios.hlsAcc160Url !== null) { audio.src = e.detail.audios.hlsAcc160Url; }
    else if (e.detail.audios.hlsMp3160Url !== null) { audio.src = e.detail.audios.hlsMp3160Url; }
    else if (e.detail.audios.httpMp3128Url !== null) { audio.src = e.detail.audios.httpMp3128Url; }
    else if (e.detail.audios.hlsOpus64Url !== null) { audio.src = e.detail.audios.hlsOpus64Url; }
    else { }

    const img = document.getElementById("image-track-bottom-panel");
    img.src = e.detail.image;

    const h4Text = document.getElementById("name-track-bottom-panel");
    h4Text.innerText = e.detail.trackName; 

    const pText = document.getElementById("artist-bottom-panel");
    pText.innerText = e.detail.artists[0].nameArtist;

    audio.addEventListener("loadedmetadata", () => {
        playerButtonPlay.disabled = false;
        playerInputTrackProgress.disabled = false;
        playerInputVolumeProgres.disabled = false;
        playerButtonNextTrack.disabled = false;
        playerButtonPreviousTrack.disabled = false;

        playerButtonPlay.style.opacity = '1';
        playerInputTrackProgress.style.opacity = '1';
        playerInputVolumeProgres.style.opacity = '1';
        playerButtonNextTrack.style.opacity = '1';
        playerButtonPreviousTrack.style.opacity = '1';
        
        let time = Math.trunc(audio.duration);
        let minutes = Math.trunc(time/60);
        let seconds = time - (minutes * 60);

        playerInputTrackProgress.max = time;

        spanTextAudioLenght.innerText = minutes + ":" + ((seconds < 10) ? "0" : "") + seconds;
    });
});
playerButtonPlay.addEventListener('click', () => {
    if (isPlay) {
        audio.pause();
    }
    else {
        audio.play();
    }
    isPlay = !isPlay;
});
playerInputVolumeProgres.addEventListener('change', function (event) {
    audio.volume = event.target.value * 0.01;
});
playerInputTrackProgress.addEventListener('change', function (event) {
    audio.currentTime = playerInputTrackProgress.value;
});
audio.addEventListener('timeupdate', () => {
    playerInputTrackProgress.value = audio.currentTime;

    let time = Math.trunc(audio.currentTime);
    let minutes = Math.trunc(time / 60);
    let seconds = time - (minutes * 60);
    let ShowTime = minutes + ":" + ((seconds<10)?"0":"") + seconds;
    spanTextProgress.innerText = ShowTime;
});
