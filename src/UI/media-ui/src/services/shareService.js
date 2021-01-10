export const shareMedia = async (media) => {

    await shareManyMedia([media])
}

export const shareManyMedia = async (medias, progress) => {

    const filesArray = new Array();

    for (let i = 0; i < medias.length; i++) {
        const media = medias[i];
        if (media.mediaType === "IMAGE") {
            let response = await fetch(`/api/download/${media.id}/SOCIAL_MEDIA`);
            let data = await response.blob();
            let metadata = {
                type: "image/jpeg",
            };
            let file = new File([data], media.filename, metadata);
            if (progress) {
                progress(file)
            }
            filesArray.push(file)
        }
    }

    if (navigator.canShare({ files: filesArray })) {
        await navigator.share({
            files: filesArray,
            title: "Images",
            text: "Magic Media",
        });
    }
}