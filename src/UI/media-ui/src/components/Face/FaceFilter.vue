<template>
  <div>
    <FilterList
      :items="persons"
      title="Person"
      max-height="250"
      value-field="id"
      text-field="name"
      @change="onSelectPerson"
    ></FilterList>

    <v-card flat>
      <v-card-text>
        <v-row align="center">
          <v-subheader>Recognition</v-subheader>
        </v-row>
        <v-row>
          <v-btn-toggle
            v-model="recognitionTypes"
            multiple
            @change="onRecognitionTypeChange"
          >
            <v-btn value="COMPUTER">
              <v-icon>mdi-server</v-icon>
            </v-btn>
            <v-btn value="HUMAN">
              <v-icon>mdi-account</v-icon>
            </v-btn>
            <v-btn value="NONE">
              <v-icon>mdi-border-none-variant</v-icon>
            </v-btn>
          </v-btn-toggle>
        </v-row>
      </v-card-text>
    </v-card>
    <v-card flat>
      <v-card-text>
        <v-row align="center">
          <v-subheader>State</v-subheader>
        </v-row>
        <v-row>
          <v-btn-toggle v-model="states" multiple @change="onStateChange">
            <v-btn value="NEW">
              <v-icon>mdi-new-box</v-icon>
            </v-btn>
            <v-btn value="PREDICTED">
              <v-icon>mdi-face-recognition</v-icon>
            </v-btn>
            <v-btn value="VALIDATED">
              <v-icon>mdi-account-check</v-icon>
            </v-btn>
          </v-btn-toggle>
        </v-row>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import FilterList from "../Common/FilterList";
export default {
  components: { FilterList },
  created() {},
  data() {
    return {
      selectedPersons: [],
      states: [],
      recognitionTypes: [],
    };
  },
  computed: {
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
    onSelectPerson: function (persons) {
      const selected = persons.map((x) => x.id);
      this.$store.dispatch("face/setPersonFilter", selected);
    },
    onRecognitionTypeChange: function () {
      this.$store.dispatch(
        "face/setRecognitionTypeFilter",
        this.recognitionTypes
      );
    },
    onStateChange: function () {
      this.$store.dispatch("face/setStateFilter", this.states);
    },
  },
};
</script>

<style scoped>
</style>
