      <template>
  <FilterList
    :items="persons"
    :open="true"
    title="Person"
    max-height="250"
    value-field="id"
    text-field="name"
    v-model="selectedPersons"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  computed: {
    selectedPersons: {
      set(value) {
        this.setFilter({
          key: "persons",
          value: value,
        });
      },
      get() {
        return this.$store.state.media.filter.persons;
      },
    },
    persons: function () {
      return this.$store.state.person.persons.map((p) => {
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