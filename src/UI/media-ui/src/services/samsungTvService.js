import apollo from "../apollo";
import QUERY_DEVICES from "../graphql/SamsungTv/GetDevices.gql";
import QUERY_MEDIAS from "../graphql/SamsungTv/GetMedias.gql";
import QUERY_FEATURES from "../graphql/SamsungTv/GetFeatures.gql";
import MUTATION_DELETE_MEDIA from "../graphql/SamsungTv/DeleteMedia.gql";
import MUTATION_SELECT_MEDIA from "../graphql/SamsungTv/SelectMedia.gql";
import MUTATION_CHANGE_MATTE from "../graphql/SamsungTv/ChangeMatte.gql";
import MUTATION_SET_FILTER from "../graphql/SamsungTv/SetFilter.gql";


export const getDevices = async () => {
  return await apollo.query({
    query: QUERY_DEVICES,
    variables: {}
  });
};

export const getMedias = async (device) => {
  return await apollo.query({
    query: QUERY_MEDIAS,
    variables: { device }
  });
};

export const getFeatures = async (device) => {
  return await apollo.query({
    query: QUERY_FEATURES,
    variables: { device }
  })
};

export const deleteMedia = async (input) => {
  return await apollo.mutate({
    mutation: MUTATION_DELETE_MEDIA,
    variables: { input }
  });
}

export const selectMedia = async (input) => {
  return await apollo.mutate({
    mutation: MUTATION_SELECT_MEDIA,
    variables: { input }
  });
}

export const changeMatte = async (input) => {
  return await apollo.mutate({
    mutation: MUTATION_CHANGE_MATTE,
    variables: { input }
  });
}

export const setFilter = async (input) => {
  return await apollo.mutate({
    mutation: MUTATION_SET_FILTER,
    variables: { input }
  });
}