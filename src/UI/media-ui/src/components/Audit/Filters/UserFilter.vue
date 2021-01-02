<template>
  <FilterList
    :items="users"
    :open="true"
    title="Users"
    max-height="250"
    value-field="id"
    text-field="name"
    :multiple="false"
    v-model="selectedUsers"
  ></FilterList>
</template>

<script>
import FilterList from "../../Common/FilterList";
import { mapActions } from "vuex";

export default {
  components: { FilterList },
  created() {
    if (this.users.length === 0) {
      this.$store.dispatch("user/getAll");
    }
  },
  computed: {
    selectedUsers: {
      set(value) {
        this.filter({
          userId: value,
        });
      },
      get() {
        return this.$store.state.audit.filter.users;
      },
    },
    users: function () {
      console.log(this.$store.state.user.all);
      return this.$store.state.user.all.map((p) => {
        return {
          name: p.name,
          id: p.id,
        };
      });
    },
  },
  methods: {
    ...mapActions("audit", ["filter"]),
  },
};
</script>

<style>
</style>