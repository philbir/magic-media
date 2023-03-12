import apollo from "../apollo";
import QUERY_FOLDER_TREE from "../graphql/Media/GetFolderTree.gql";
import QUERY_GETBYID from "../graphql/Media/GetMediaDetails.gql";
import QUERY_METADATA from "../graphql/Media/MediaMetadata.gql";
import QUERY_INFO from "../graphql/Media/GetMediaInfo.gql";
import MUTATION_MOVE_MEDIA from "../graphql/Media/MoveMedia.gql";
import MUTATION_RECYCLE_MEDIA from "../graphql/Media/RecycleMedia.gql";
import MUTATION_DELETE_MEDIA from "../graphql/Media/DeleteMedia.gql";
import MUTATION_EXPORT_MEDIA from "../graphql/Media/ExportMedia.gql";
import MUTATION_QUICK_EXPORT_MEDIA from "../graphql/Media/QuickExportMedia.gql";
import QUERY_SEARCH from "../graphql/Media/SearchMedia.gql";
import MUTATION_TOGGLE_FAVORITE from "../graphql/Media/ToggleFavorite.gql";
import MUTATION_UPDATE_METADATA from "../graphql/Media/UpdateMediaMetadata.gql";
import MUTATION_RESCAN_FACES from "../graphql/Media/ReScanFaces.gql";
import MUTATION_ANALYSE_MEDIA from "../graphql/Media/AnalyseMedia.gql";
import QUERY_GEO_LOCATION_CLUSTERS from "../graphql/Media/geoLocationClusters.gql";
import QUERY_SIMILAR_GROUPS from "../graphql/Media/GetSimilarGroups.gql"
import QUERY_SEARCH_FACETS from "../graphql/SearchFacets.gql";
import QUERY_CONSISTENCY_REPORT from "../graphql/Media/GetMediaConsistencyReport.gql"


export const searchMedia = async (request, size, loadThumbnailData) => {
  return await apollo.query({
    query: QUERY_SEARCH,
    variables: {
      request: request,
      size: size,
      loadThumbnailData: loadThumbnailData
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

export const deleteMedia = async input => {
  return await apollo.mutate({
    mutation: MUTATION_DELETE_MEDIA,
    variables: {
      input: input
    }
  });
};

export const exportMedia = async input => {
  return await apollo.mutate({
    mutation: MUTATION_EXPORT_MEDIA,
    variables: {
      input: input
    }
  });
};

export const reScanFaces = async input => {
  return await apollo.mutate({
    mutation: MUTATION_RESCAN_FACES,
    variables: {
      input: input
    }
  });
};


export const quickExportMedia = async input => {
  return await apollo.mutate({
    mutation: MUTATION_QUICK_EXPORT_MEDIA,
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

export const analyseMedia = async (id) => {
  return await apollo.mutate({
    mutation: MUTATION_ANALYSE_MEDIA,
    variables: {
      input: {
        id: id,
      }
    }
  });
};

export const getSimilarGroups = async (request) => {
  return await apollo.query({
    query: QUERY_SIMILAR_GROUPS,
    variables: {
      request: request,
    }
  });
};

export const getConsistencyReport = async (id) => {
  return await apollo.query({
    query: QUERY_CONSISTENCY_REPORT,
    variables: {
      id: id,
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
