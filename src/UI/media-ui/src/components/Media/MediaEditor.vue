<template>
  <FilerobotImageEditor
    @saved="handleSaved"
    @close="handleClose"
    :src="src"
  ></FilerobotImageEditor>
</template>

<script>
import FilerobotImageEditor from "../Common/FilerobotImageEditor";

export default {
  components: { FilerobotImageEditor },
  props: {
    mediaId: String
  },
  data() {
    return {
      media: null,
      loading: false,
      src: `/api/media/original/${this.$route.params.id}`,
      colorScheme: "light"
    };
  },
  created() {},
  mounted() {},
  computed: {},
  methods: {
    handleSaved(file) {
      if (file.name.startsWith(this.$route.params.id)) {
        var formData = new FormData();
        formData.append("file", file);

        fetch(`/api/media/editor/save/${this.$route.params.id}`, {
          method: "POST",
          body: formData
        })
          .then(response => response.text())
          .then(responseText => {
            console.log(responseText);
          });
      } else {
        var url = window.URL.createObjectURL(file);
        window.location.assign(url);
      }
    },
    handleClose() {
      this.$router.back();
    },
    onBeforeComplete(element) {
      if (element && element.canvas) {
        this.src = element.canvas.toDataURL();
      }
    },
    onError(error) {
      console.log(" error " + error);
    }
  }
};
</script>
