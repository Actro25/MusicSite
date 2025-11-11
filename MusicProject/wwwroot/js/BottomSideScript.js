const playerButtonPlay = document.getElementById("play-track-button");
const playerInputTrackProgress = document.getElementById("play-track-input");
const playerInputVolumeProgres = document.getElementById("volume-track-input");
const audio = document.getElementById("track-player");
let isPlay = false;
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

    const spanTextAudioLenght = document.getElementById("end-lenght-track");
    spanTextAudioLenght.innerText = audio.duration;
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
});
