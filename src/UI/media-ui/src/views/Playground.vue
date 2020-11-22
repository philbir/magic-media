<template>
  <div class="player-container">
    <vue-plyr>
      <video controls crossorigin playsinline>
        <source
          src="/api/video/6b6d9023d7764d4aa39c289996e5cb23"
          type="video/mp4"
        />
      </video>
    </vue-plyr>
  </div>
</template>

<script>
export default {};
</script>

<style>
.player-container {
  height: 80vh;
  background-color: #000;
  font-weight: normal;
}
</style>