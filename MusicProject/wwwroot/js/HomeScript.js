const homeFirstPageVideo = document.getElementById("home-first-page");
const buttonSND = document.getElementById("sound-button");

// Завантажуємо анімацію після завантаження DOM
let homeFirstPageAnimation;

document.addEventListener("DOMContentLoaded", function() {
    homeFirstPageAnimation = bodymovin.loadAnimation({
        container: document.getElementById('home-animation-first-page'),
        path: '/json/CircleAnimation.json', // Змінив шлях
        render: 'svg',
        loop: true,
        autoplay: true,
        name: 'circle animation'
    });
});

buttonSND.addEventListener("click", function (event) {
    homeFirstPageVideo.muted = !homeFirstPageVideo.muted;
    console.log(homeFirstPageVideo);
    ChangePngOnSoundButton();
});

function ChangePngOnSoundButton() {
    if (homeFirstPageVideo.muted) {
        // Дія коли звук вимкнено
        console.log("Sound is muted");
    } else {
        // Дія коли звук увімкнено
        console.log("Sound is unmuted");
    }
}