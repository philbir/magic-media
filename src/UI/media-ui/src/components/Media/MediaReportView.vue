<template>
  <v-sheet elevation="10" rounded="lg" class="ma-2 pa-4 card-details-content">
    <v-row dense>
      <v-col sm="6">
        <h2>Consistency checks</h2>
        <v-expansion-panels class="mt-2" v-if="report">
          <v-expansion-panel v-for="(check, i) in report.checks" :key="i">
            <v-expansion-panel-header disable-icon-rotate>
              <b>{{ check.name }}</b>
              <template v-slot:actions>
                <v-icon color="teal" v-if="check.success">
                  mdi-check
                </v-icon>
                <v-icon color="error" v-else>
                  mdi-alert-circle
                </v-icon>
              </template>
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <div v-for="data in check.data" :key="data.name">
                <div>
                  {{ data.name }}: <strong>{{ data.value }}</strong>
                </div>
              </div>
              <div class="ma-2" v-if="check.repairs.length > 0">
                <h4>Repairs</h4>
                <div v-for="repair in check.repairs" :key="repair.type">
                  <v-row dense>
                    <v-col sm="8">
                      <div v-for="par in repair.parameters" :key="par.name">
                        <div v-if="par.name == 'Found_Image'">
                          <v-img
                            :src="par.value"
                            max-width="300"
                            contain
                          ></v-img>
                        </div>
                        <div v-else>
                          {{ par.name }}: <strong>{{ par.value }}</strong>
                        </div>
                      </div>
                    </v-col>
                    <v-col sm="4">
                      <v-btn
                        class="ma-2"
                        color="primary"
                        variant="outlined"
                        elevation="4"
                      >
                        {{ repair.title }}
                        <v-icon right>mdi-wrench-outline</v-icon>
                      </v-btn>
                    </v-col>
                  </v-row>
                </div>
              </div>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
        <v-progress-circular
          class="ma-4"
          v-else
          indeterminate
          color="primary"
        ></v-progress-circular>
      </v-col>
      <v-col sm="6"></v-col>
    </v-row>
  </v-sheet>
</template>
<script>
import "vue-json-pretty/lib/styles.css";

export default {
  components: {},
  props: {
    report: {
      type: Object,
      default: null
    }
  },
  data() {
    return {};
  }
};
</script>
