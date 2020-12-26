      <template>
  <FilterList
    v-if="groups"
    :items="groups"
    :open="true"
    title="Group"
    max-height="250"
    value-field="id"
    text-field="name"
    v-model="selectedGroups"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedGroups: {
      set(value) {
        this.setFilter({
          key: "groups",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.groups;
      },
    },
    groups: function () {
      return this.$store.state.person.groups.map((p) => {
        return {
          name: p.name,
          id: p.id,
        };
      });
    },
  },
  methods: {
    ...mapActions("media", ["setFilter"]),
  },
};
</script>

<style>
</style>