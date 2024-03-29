
import { mediaOperationTypeMap } from "../services/mediaOperationService";

const snackbarModule = {
  namespaced: true,
  state: () => ({
    snacks: [],
    notifications: []
  }),
  mutations: {
    SNACK_ADDED: function (state, snack) {
      state.snacks.push(snack);
    },
    OPERATION_ADDED: function (state, operation) {
      state.notifications.push({
        id: operation.id,
        type: operation.type,
        title: operation.title,
        totalCount: operation.totalCount,
        successCount: 0,
        errorCount: 0,
        text: `${0} of ${operation.totalCount} ${mediaOperationTypeMap[operation.operationType].verb}`,
        active: true
      });
    },
    OPERATION_UPDATED: function (state, operation) {
      const idx = state.notifications.findIndex(x => x.id === operation.id);
      if (idx > -1) {
        state.notifications[idx] = operation;
      }
    }
  },
  actions: {
    addSnack: function ({ commit }, snack) {
      snack.show = true;
      commit("SNACK_ADDED", snack);
    },
    addNotification: function ({ commit }, notification) {
      commit("NOTIFICATION_ADDED", notification);
    },
    operationStarted: function ({ commit }, operation) {
      commit("OPERATION_ADDED", operation);
    },
    mediaOperationCompleted: function ({ state, commit }, operation) {
      const ntf = state.notifications.find(x => x.id === operation.operationId);
      if (ntf) {
        if (operation.isSuccess) {
          ntf.successCount++;
        } else {
          ntf.errorCount++;
        }

        var opType = mediaOperationTypeMap[operation.type];

        ntf.text = `${ntf.successCount} of ${ntf.totalCount} ${opType.verb}`;
        if (ntf.errorCount > 0) {
          ntf.text += ` (${ntf.errorCount} errors)`;
        }
        commit("OPERATION_UPDATED", ntf);
      }
    },
    mediaOperationRequestCompleted: function ({ state, commit }, operation) {
      const ntf = state.notifications.find(x => x.id === operation.operationId);
      if (ntf) {
        if (operation.errorCount === 0) {
          ntf.type = "success";
        } else {
          ntf.type = "warning";
        }
        var opType = mediaOperationTypeMap[operation.type];

        ntf.successCount = operation.successCount;
        ntf.errorCount = operation.errorCount;
        ntf.text = `${ntf.successCount} of ${ntf.totalCount} ${opType.verb}.`;
        commit("OPERATION_UPDATED", ntf);
      }
    }
  },
  getters: {}
};

export default snackbarModule;
