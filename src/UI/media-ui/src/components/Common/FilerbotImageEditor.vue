<template>
    <div style="height: 100vh, width: 100%" id="editor_container" />
</template>

<script>
import FilerobotImageEditor from 'filerobot-image-editor';

const { TABS, TOOLS } = FilerobotImageEditor;

export default {
    props: {
        src: {
            type: String,
            required: true,
        },
        config: {
            type: Object,
            default: () => ({}),
        },
    },
    mounted() {
        this.init();
    },
    methods: {
        init() {

            const config = {
                source: this.src,
                onSave: (editedImageObject, designState) => {
                    editedImageObject.imageCanvas.toBlob((blob) => {
                        const file = new File([blob], editedImageObject.fullName, { type: editedImageObject.mimeType });
                        this.$emit("saved", file, designState);
                    }, editedImageObject.mimeType)
                },
                annotationsCommon: {
                    fill: '#ff0000',
                },
                observePluginContainerSize: false,
                Text: { text: 'Yeah' },
                defaultSavedImageType: "jpg",
                savingPixelRatio: 10,
                Rotate: { angle: 90, componentType: 'buttons' },
                Crop: {
                    presetsItems: [
                        {
                            titleKey: 'Samsung Frame',
                            descriptionKey: '16:9',
                            ratio: 19 / 9,
                        },
                    ],
                },
                //tabsIds: [TABS.ADJUST, TABS.ANNOTATE, TABS.WATERMARK],
                defaultTabId: TABS.ADJUST,
                defaultToolId: TOOLS.TEXT, // or 'Text',
            }


            this.editor = new FilerobotImageEditor(
                document.querySelector('#editor_container'),
                config,
            );
            this.editor.render({
                onClose: (reason) => {
                    this.$emit("close", reason);
                    this.editor.terminate();
                },
            });

            window.setTimeout(() => {
                document.querySelector("#editor_container .konvajs-content").style.height = (this.$vuetify.breakpoint.height -200) + "px";
            }, 1500)
        },
    },
    destroy() {
        if (this.editor) {
            this.editor.unmounted();
        }
    },
};
</script>
<style scoped>
.editor-container {
    height: 95vh;
    width: 100%;
}
</style>
