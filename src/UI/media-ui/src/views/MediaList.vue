<template>
  <ApolloQuery :query="gql.search">
    <template slot-scope="{ result: { loading, error, data } }">
      <!-- Loading -->
      <div v-if="loading" class="loading Media">Loading...</div>

      <!-- Error -->
      <div v-else-if="error" class="error apollo">An error occured</div>

      <!-- Result -->
      <div v-else-if="data">
        <div class="media-container" style="height: 1000px">
          <div
            v-for="(box, i) in getLayout(data.searchMedia).boxes"
            :key="i"
            class="media-item"
            v-on:click="
              $router.push({ name: 'media', params: { id: box.media.id } })
            "
            :style="{
              left: box.left + 'px',
              top: box.top + 'px',
              height: box.height + 'px',
              width: box.width + 'px',
              'background-image': 'url(' + box.media.thumbnail.dataUrl + ')',
            }"
          />
        </div>
      </div>

      <!-- No result -->
      <div v-else class="no-result apollo">No result :(</div>
    </template>
  </ApolloQuery>
</template>

<script>
import QUERY_SEARCH from "../graphql/SearchMedia.gql";
import justified from "justified-layout";
import { mediaListViewMap } from "../services/mediaListViewMap";

export default {
  data() {
    return {
      windowWidth: window.innerWidth,
      gql: {
        search: QUERY_SEARCH,
      },
    };
  },
  methods: {
    getLayout(items) {
      if (items) {
        const viewMap = mediaListViewMap["l"];
        const ratios = [];
        items.forEach((item) => {
          ratios.push(
            viewMap.fixedRatio > 0
              ? viewMap.fixedRatio
              : item.dimension.width / item.dimension.height
          );
        });
        const layout = justified(ratios, {
          containerWidth: window.innerWidth,
          targetRowHeight: viewMap.rowHeight,
          boxSpacing: viewMap.spacing,
          containerPadding: viewMap.spacing,
        });

        layout.boxes.forEach((box, i) => {
          box.media = items[i];
        });
        console.log(layout);
        return layout;
      }

      return items;
    },
  },
};
</script>

<style scoped>
.media-container {
  position: relative;
  overflow-y: scroll;
}
.media-item {
  position: absolute;
  background-repeat: no-repeat;
  background-size: 100% 100%;
}
</style>
