<template>
  <v-data-table
    :headers="headers"
    :items="events"
    :options.sync="options"
    :server-items-length="totalCount"
    :loading="loading"
    :height="$vuetify.breakpoint.height - 110"
    :footer-props="{ itemsPerPageOptions: [50, 100, 200] }"
    fixed-header
    class="elevation-1"
  >
    <template v-slot:item.id="{ item }">
      <img v-if="item.thumbnail" class="thumb" :src="item.thumbnail" />
    </template>
    <template v-slot:item.success="{ item }">
      <v-icon
        :color="item.success ? 'green' : 'red'"
        v-text="item.success ? 'mdi-check' : 'mdi-close'"
      ></v-icon>
    </template>
    <template v-slot:item.timestamp="{ item }">
      {{ item.timestamp | dateformat("DATETIME_SHORT") }}
    </template>
    <template v-slot:item.ipInfo="{ item }">
      <div v-if="item.thumbprint && item.thumbprint.ipInfo.city">
        {{
          `${item.thumbprint.ipInfo.city} (${item.thumbprint.ipInfo.ipAddress})`
        }}
      </div>
      <span v-else-if="item.thumbprint">{{
        item.thumbprint.ipInfo.ipAddress
      }}</span>
    </template>
  </v-data-table>
</template>

<script>
import { mapActions, mapState } from "vuex";
export default {
  data() {
    return {
      options: {},
      headers: [
        {
          text: "Success",
          align: "start",
          sortable: false,
          value: "success",
          width: 30,
        },
        {
          text: "Timestamp",
          align: "start",
          sortable: false,
          value: "timestamp",
          width: 180,
        },
        {
          text: "Resource",
          sortable: false,
          value: "resource.type",
          width: 80,
        },
        {
          text: "User",
          sortable: false,
          value: "user.name",
          width: 180,
        },
        { text: "Action", value: "action", width: 80, sortable: false },
        { text: "IP", value: "ipInfo", sortable: false },
        {
          text: "Agent",
          value: "thumbprint.userAgent.description",
          sortable: false,
        },
        {
          text: "Media",
          align: "start",
          value: "id",
          width: 56,
          sortable: false,
        },
      ],
    };
  },
  watch: {
    options: {
      handler() {
        this.setPaging({
          pageSize: this.options.itemsPerPage,
          pageNr: this.options.page - 1,
        });
      },
      deep: true,
    },
  },
  mounted() {
    this.search();
  },
  computed: {
    ...mapState("audit", {
      events: "list",
      totalCount: "totalCount",
      loading: "listLoading",
    }),
  },
  methods: {
    ...mapActions("audit", ["search", "setPaging"]),
    async getEvents() {
      this.search();
    },
  },
};
</script>

<style>
.thumb {
  height: 34px;
  width: 34px;
  border-radius: 8px;
}
</style>