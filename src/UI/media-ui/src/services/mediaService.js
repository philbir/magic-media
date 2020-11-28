import apollo from "../apollo";
import QUERY_FOLDER_TREE from "../graphql/Media/GetFolderTree.gql";
import QUERY_GETBYID from "../graphql/Media/GetMediaDetails.gql";
import QUERY_METADATA from "../graphql/Media/MediaMetadata.gql";
import QUERY_INFO from "../graphql/Media/GetMediaInfo.gql";
import MUTATION_MOVE_MEDIA from "../graphql/Media/MoveMedia.gql";
import MUTATION_RECYCLE_MEDIA from "../graphql/Media/RecycleMedia.gql";
import QUERY_SEARCH from "../graphql/Media/SearchMedia.gql";
import MUTATION_TOGGLE_FAVORITE from "../graphql/Media/ToggleFavorite.gql";
import MUTATION_UPDATE_METADATA from "../graphql/Media/UpdateMediaMetadata.gql";
import QUERY_GEO_LOCATION_CLUSTERS from "../graphql/Media/geoLocationClusters.gql";
import QUERY_SEARCH_FACETS from "../graphql/SearchFacets.gql";

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

export const getInfo = async id => {
  return await apollo.query({
    query: QUERY_INFO,
    variables: {
      id: id
    }
  });
};


export const getMetadata = async id => {
  return await apollo.query({
    query: QUERY_METADATA,
    variables: {
      id: id
    }
  });
};

export const getGeoLocationClusters = async input => {
  return await apollo.query({
    query: QUERY_GEO_LOCATION_CLUSTERS,
    variables: {
      input: input
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

export const recycleMedia = async input => {
  return await apollo.mutate({
    mutation: MUTATION_RECYCLE_MEDIA,
    variables: {
      input: input
    }
  });
};
export const updateMetadata = async input => {
  return await apollo.mutate({
    mutation: MUTATION_UPDATE_METADATA,
    variables: {
      input
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

export const parsePath = path => {
  const parts = path.split("/");
  const len = parts.length;
  const pathInfo = [];
  let fullPath = "";
  for (let i = 0; i < len; i++) {
    if (parts[i].length > 0) {
      fullPath += "/" + parts[i];
      pathInfo.push({
        path: fullPath.substr(1),
        name: parts[i]
      });
    }
  }

  return pathInfo;
};
