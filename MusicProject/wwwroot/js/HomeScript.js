const homeFirstPageVideo = document.getElementById("home-first-page");
const buttonSND = document.getElementById("sound-button");

let homeFirstPageAnimation;

document.addEventListener("DOMContentLoaded", function() {
    homeFirstPageAnimation = bodymovin.loadAnimation({
        container: document.getElementById('home-animation-first-page'),
        path: '/json/CircleAnimation.json',
        render: 'svg',
        loop: true,
        autoplay: true,
        name: 'circle animation'
    });
    homeFirstPageMusicPlay = bodymovin.loadAnimation({
        container: document.getElementById('home-animation-first-page-music'),
        path: '/json/Recording.json',
        render: 'svg',
        loop: true,
        autoplay: false
    });
});

buttonSND.addEventListener("click", function (event) {
    homeFirstPageVideo.muted = !homeFirstPageVideo.muted
    ChangePngOnSoundButton();
});

function ChangePngOnSoundButton() {
    if (homeFirstPageVideo.muted) {
        homeFirstPageMusicPlay.stop();
    } else {
        homeFirstPageMusicPlay.play();
    }
}
