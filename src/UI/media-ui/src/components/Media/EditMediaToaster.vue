<template>
    <div class="editor-container">
        <tui-image-editor ref="editor" :include-ui="true" :options="options"></tui-image-editor>

    </div>
</template>


<script>
import 'tui-color-picker/dist/tui-color-picker.css';
import 'tui-image-editor/dist/tui-image-editor.css';

import { ImageEditor } from '@toast-ui/vue-image-editor';
export default {
    components: { "tui-image-editor": ImageEditor },
    props: {
        mediaId: String
    },
    data() {
        return {
            media: null,
            loading: false,
            useDefaultUI: true,
            options: {
                includeUI: {
                    loadImage: {
                        path: `/api/media/original/${this.$route.params.id}`,
                        name: 'Foo',
                    },
                    menu: [ 'rotate','crop', 'flip', 'draw', 'shape', 'icon', 'text', 'mask', 'filter'],
                },

            },
        };
    },
    created() {

    },
    mounted() {
        //his.$refs.editor.invoke('loadImageFromURL', `/api/media/original/${this.$route.params.id}`, 'Edit');
    },
    computed: {

    },
    methods: {

    }
};
</script>

<style scoped>
.editor-container {
    height: 95vh;
    width: 100%;
}
</style>