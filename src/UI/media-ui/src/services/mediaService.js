import apollo from "../apollo";
import QUERY_FOLDER_TREE from "../graphql/GetFolderTree.gql";
import QUERY_GETBYID from "../graphql/GetMediaDetails.gql";
import QUERY_SEARCH_FACETS from "../graphql/SearchFacets.gql";
import QUERY_SEARCH from "../graphql/SearchMedia.gql";
import MUTATION_MOVE_MEDIA from "../graphql/MoveMedia.gql";

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

export const moveMedia = async (request) => {
  return await apollo.query({
    query: MUTATION_MOVE_MEDIA,
    variables: {
      request: request
    }
  });
};

export const subscribeOperationCompleted = async (id) => {
  return await apollo.subscribe({
    query: MUTATION_MOVE_MEDIA,
    variables: {
      operationId: id
    }
  }).subscribe({
    next(data) {
      console.log(data)
    }
  });
};


