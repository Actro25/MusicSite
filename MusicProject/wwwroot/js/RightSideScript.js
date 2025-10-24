const rightPanel = document.getElementById("resizable-right");
const rightHandle = document.getElementById("resize-line-right");

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
    console.log(resizingRight);
    if (!resizingRight) return;

    // Виправлена логіка розрахунку
    const deltaX = startXright - e.clientX;
    const newWidth = startWidthright + deltaX;

    const minWidth = 300;
    const maxWidth = 400;

    if (newWidth >= minWidth && newWidth <= maxWidth) {
        rightPanel.style.width = newWidth + "px";
    }
});

document.addEventListener("mouseup", () => {
    if (!resizingRight) return;

    resizingRight = false;
    document.body.style.cursor = "";
    document.body.style.userSelect = "";

    rightHandle.style.background = "";
    rightHandle.style.opacity = "";
});