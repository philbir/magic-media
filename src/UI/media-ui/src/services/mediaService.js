import apollo from "../apollo";
import QUERY_FOLDER_TREE from "../graphql/GetFolderTree.gql";
import QUERY_GETBYID from "../graphql/GetMediaDetails.gql";
import MUTATION_MOVE_MEDIA from "../graphql/MoveMedia.gql";
import QUERY_SEARCH_FACETS from "../graphql/SearchFacets.gql";
import QUERY_SEARCH from "../graphql/SearchMedia.gql";
import SUBSCRIPTION_OPERATION_COMPLETED from "../graphql/SubscribeOperationCompleted.gql";
import MUTATION_TOGGLE_FAVORITE from "../graphql/ToggleFavorite.gql";

/* eslint-disable no-debugger */
export const searchMedia = async (request, size) => {
  return await apollo.query({
    query: QUERY_SEARCH,
    variables: {
      request: request,
      size: size
    }
  });
};

export const getById = async id => {
  return await apollo.query({
    query: QUERY_GETBYID,
    variables: {
      id: id
    }
  });
};

export const getSearchFacets = async () => {
  return await apollo.query({
    query: QUERY_SEARCH_FACETS,
    variables: {}
  });
};

export const getFolderTree = async () => {
  return await apollo.query({
    query: QUERY_FOLDER_TREE,
    variables: {}
  });
};

export const moveMedia = async request => {
  return await apollo.mutate({
    mutation: MUTATION_MOVE_MEDIA,
    variables: {
      request: request
    }
  });
};

export const toggleFavorite = async (id, isFavorite) => {
  return await apollo.mutate({
    mutation: MUTATION_TOGGLE_FAVORITE,
    variables: {
      input: {
        id: id,
        isFavorite: isFavorite
      }
    }
  });
};

export const subscribeOperationCompleted = async id => {
  console.log(id);
  return apollo
    .subscribe({
      query: SUBSCRIPTION_OPERATION_COMPLETED,
      variables: {
        name: "MediaOperation"
      }
    })
    .subscribe({
      next(data) {
        console.log("GQL_SUB", data);
      },
      error(err) {
        console.log(err);
      },
      complete(c) {
        console.log(c);
      }
    });
};
