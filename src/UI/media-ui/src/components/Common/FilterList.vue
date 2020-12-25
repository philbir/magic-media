<template>
  <v-card tile flat v-if="items.length > 0">
    <v-subheader class="my-0 py-0" v-if="!showFilter">{{ title }}</v-subheader>
    <v-card-text class="my-0 py-0">
      <v-row v-if="showFilter" class="my-0 py-0">
        <v-col sm="10" class="my-0 py-0">
          <v-text-field
            v-model="searchText"
            :label="title"
            prepend-inner-icon="mdi-magnify"
            @click="expanded = true"
          ></v-text-field>
        </v-col>
        <v-col sm="2">
          <v-icon @click="toogleOpen" v-text="expandIcon"></v-icon>
        </v-col>
      </v-row>

      <v-list flat dense v-if="expanded">
        <v-list-item-group
          v-model="selected"
          @change="onChange"
          :multiple="multiple"
        >
          <v-virtual-scroll
            :bench="2"
            :items="filtered"
            :height="maxHeight"
            item-height="38"
          >
            <template v-slot:default="{ item }">
              <v-list-item
                v-show="item.visible"
                dense
                :key="item[valueField]"
                :value="item[valueField]"
              >
                <template v-slot:default="{ active }">
                  <v-list-item-action class="pa-0">
                    <v-checkbox
                      :input-value="active"
                      color="primary"
                    ></v-checkbox>
                  </v-list-item-action>
                  <v-list-item-content>
                    <slot v-bind:item="item">
                      <v-list-item-title>{{ item.display }} </v-list-item-title>
                    </slot>
                  </v-list-item-content>
                </template>
              </v-list-item>
            </template>
          </v-virtual-scroll>
        </v-list-item-group>
      </v-list>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  props: {
    items: Array,
    value: [String, Array, Number],
    open: {
      type: Boolean,
      default: false,
    },
    multiple: {
      type: Boolean,
      default: true,
    },
    maxHeight: {
      type: [String, Number],
    },
    title: String,
    valueField: {
      type: String,
      default: "value",
    },
    textField: {
      type: String,
      default: "text",
    },
  },
  mounted() {
    console.log("Show filter:", this.showFilter);
    this.expanded = this.open || !this.showFilter;
  },
  data() {
    return {
      searchText: "",
      expanded: false,
    };
  },
  computed: {
    selected: {
      get() {
        return this.value;
      },
      set(val) {
        this.$emit("input", val);
      },
    },
    showFilter: function () {
      return this.items.length > 5;
    },
    filtered: function () {
      var self = this;
      const mapped = this.items.map((x) => {
        x.visible = x[self.textField]
          .toLowerCase()
          .includes(this.searchText.toLowerCase());
        x.display = x.count
          ? `${x[self.textField]} (${x.count})`
          : x[self.textField];
        return x;
      });

      return mapped.filter((x) => x.visible);
    },
    expandIcon: function () {
      return `mdi-chevron-${this.expanded ? "up" : "down"}`;
    },
  },
  methods: {
    onChange: function () {
      this.$emit("change", this.selected);
    },
    toogleOpen: function () {
      this.expanded = !this.expanded;
    },
  },
};
</script>

<style>
</style>
