<template>
  <v-menu offset-y class="d-sm-none-and-down">
    <template v-slot:activator="{ on, attrs }">
      <v-badge
        :value="activeNotifications.length > 0"
        :content="activeNotifications.length"
        color="red"
        overlap
      >
        <v-icon color="white" v-bind="attrs" v-on="on" class="mr-2 ml-2">
          mdi-bell-outline
        </v-icon>
      </v-badge>
    </template>

    <v-list width="360" v-if="activeNotifications.length > 0">
      <template v-for="ntf in activeNotifications">
        <v-list-item :key="ntf.id">
          <v-list-item-content>
            <v-alert
              dismissible
              :type="ntf.type.toLowerCase()"
              v-model="ntf.active"
              @
            >
              {{ ntf.text }}</v-alert
            >
          </v-list-item-content>
        </v-list-item>
      </template>
    </v-list>
  </v-menu>
</template>

<script>
export default {
  data: () => ({}),
  computed: {
    count: function () {
      return 10;
    },
    activeNotifications: function () {
      return this.$store.state.snackbar.notifications.filter((x) => x.active);
    },
  },
};
</script>

<style>
</style>
