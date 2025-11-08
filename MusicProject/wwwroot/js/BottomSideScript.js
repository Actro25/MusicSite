window.addEventListener('BottomSidePanelReceive', (e) => {
    const img = document.getElementById("image-track-bottom-panel");
    img.src = e.detail.image;

    const h4Text = document.getElementById("name-track-bottom-panel");
    h4Text.innerText = e.detail.trackName; 

    const pText = document.getElementById("artist-bottom-panel");
    pText.innerText = e.detail.artists[0].nameArtist;
});