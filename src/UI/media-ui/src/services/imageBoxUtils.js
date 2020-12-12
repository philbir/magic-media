
export const getCssBox = (item, image, topOffset = 0, showTitleThreshold = 30) => {

    const box = {};

    const ratio = image.naturalWidth / image.width;
    box.left = Math.round(item.left / ratio) + image.offsetLeft;
    box.top = Math.round(item.top / ratio) + image.offsetTop;
    box.width = Math.round((item.right - item.left) / ratio);
    box.height = Math.round((item.bottom - item.top) / ratio);

    if (topOffset > 0 && box.width >= showTitleThreshold) {
        box.height = box.height + topOffset;
        box.top = box.top - topOffset;
        box.showTitle = true;
    }

    return box;

}