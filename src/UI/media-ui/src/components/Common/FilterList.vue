<template>
  <v-card tile flat>
    <v-card-title v-if="!showFilter">
      <v-subheader>{{ title }}</v-subheader>
    </v-card-title>
    <v-card-text>
      <v-row v-if="showFilter">
        <v-col>
          <v-text-field
            v-model="searchText"
            :label="title"
            prepend-inner-icon="mdi-magnify"
          ></v-text-field>
        </v-col>
      </v-row>

      <v-list
        flat
        dense
        :max-height="maxHeight"
        width="250"
        class="overflow-y-auto"
      >
        <v-list-item-group v-model="selected" @change="onChange" multiple>
          <v-list-item
            v-for="item in filtered"
            v-show="item.visible"
            dense
            :key="item.value"
            :value="item"
          >
            <template v-slot:default="{ active }">
              <v-list-item-action>
                <v-checkbox
                  :input-value="active"
                  :true-value="item[valueField]"
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
        </v-list-item-group>
      </v-list>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  props: {
    items: Array,
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

  data() {
    return {
      searchText: "",
      selected: [],
    };
  },
  computed: {
    showFilter: function () {
      return this.items.length > 5;
    },
    filtered: function () {
      var self = this;
      return this.items.map((x) => {
        x.visible = x[self.textField]
          .toLowerCase()
          .includes(this.searchText.toLowerCase());
        x.display = x.count
          ? `${x[self.textField]} (${x.count})`
          : x[self.textField];
        return x;
      });
    },
  },
  methods: {
    onChange: function () {
      this.$emit("change", this.selected);
    },
  },
};
</script>

<style>
</style>
