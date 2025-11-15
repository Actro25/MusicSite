const rightPanel = document.getElementById("resizable-right");
const rightHandle = document.getElementById("resize-line-right");
const rightPanelArtistDiv = document.getElementById("track-artists-div");

let resizingRight = false;
let startXright, startWidthright;

rightHandle.addEventListener("mousedown", (e) => {
    resizingRight = true;
    startXright = e.clientX;
    startWidthright = parseInt(document.defaultView.getComputedStyle(rightPanel).width, 10);
    document.body.style.cursor = "w-resize";
    document.body.style.userSelect = "none";

    e.preventDefault();
    e.stopPropagation(); 
});

document.addEventListener("mousemove", (e) => {
    if (!resizingRight) return;

    const deltaX = startXright - e.clientX;
    const newWidth = startWidthright + deltaX;

    const minWidth = 300;
    const maxWidth = 400;

    if (newWidth >= minWidth && newWidth <= maxWidth) {
        rightPanel.style.width = newWidth + "px";
    }
});
window.addEventListener("RightSidePanelReceive", (e) => {
    rightPanelArtistDiv.innerHTML = '';
    const h4Text = document.createElement('h4');
    h4Text.innerText = "Artists";

    const img = document.getElementById("track-image-right-panel");
    img.src = e.detail.image;


    const aHrefForTrackName = document.getElementById("a-href-track-name");
    aHrefForTrackName.href = e.detail.trackUrl;
    const h2Text = document.getElementById("track-name-right-panel");
    h2Text.innerText = e.detail.trackName;


    
    Object.values(e.detail.artists).forEach(artist => {
        const pText = document.createElement('p');
        const aHrefToSpotifyArtist = document.createElement('a');
        const divForTextAndHref = document.createElement('div'); 

        divForTextAndHref.width = '100%';
        divForTextAndHref.style.display = 'flex';
        divForTextAndHref.style.justifyContent = 'space-between';
        divForTextAndHref.style.allignItems = 'center';

        pText.innerText = artist.nameArtist;

        aHrefToSpotifyArtist.href = artist.urlArtist;
        aHrefToSpotifyArtist.innerText = "In Origin";

        divForTextAndHref.appendChild(pText);
        divForTextAndHref.appendChild(aHrefToSpotifyArtist);

        rightPanelArtistDiv.appendChild(divForTextAndHref);
        rightPanelArtistDiv.appendChild(divForTextAndHref);
    });
    //pText.innerText = e.detail.artists[0].nameArtist;
    //h4Text.innerText = e.detail.artists.map(a => a.nameArtist).join(", ");
})
document.addEventListener("mouseup", () => {
    if (!resizingRight) return;

    resizingRight = false;
    document.body.style.cursor = "";
    document.body.style.userSelect = "";

    rightHandle.style.background = "";
    rightHandle.style.opacity = "";
});