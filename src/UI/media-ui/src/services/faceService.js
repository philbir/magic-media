import apollo from "../apollo";
import MUTATION_APPROVE_ALL_BY_MEDIA from "../graphql/Face/ApproveAllFacesByMedia.gql";
import MUTATION_APPROVE_FACE from "../graphql/Face/ApproveFaceComputer.gql";
import MUTATION_ASSIGN_PERSON from "../graphql/Face/AssignPersonByHuman.gql";
import MUTATION_BUILD_PERSON_MODEL from "../graphql/Face/BuildPersonModel.gql";
import MUTATION_DELETE_FACE from "../graphql/Face/DeleteFace.gql";
import MUTATION_DELETE_UNASSIGNED_BY_MEDIA from "../graphql/Face/DeleteUnassignedFacesByMedia.gql";
import MUTATION_PREDICT_PERSON from "../graphql/Face/PredictPerson.gql";
import MUTATION_PREDICT_PERSONS_BY_MEDIA from "../graphql/Face/PredictPersonsByMedia.gql";
import QUERY_SEARCH_FACES from "../graphql/Face/SearchFaces.gql";
import MUTATION_UNASSIGN_PERSON from "../graphql/Face/UnassignPersonFromFace.gql";
import MUTATION_UNASSIGN_PREDICTED_BY_MEDIA from "../graphql/Face/UnassignAllPredictedByMedia.gql"

export const searchFaces = async request => {
  return await apollo.query({
    query: QUERY_SEARCH_FACES,
    variables: {
      request: request
    }
  });
};

export const assignPerson = async (faceId, personName) => {
  return await apollo.mutate({
    mutation: MUTATION_ASSIGN_PERSON,
    variables: {
      input: {
        faceId: faceId,
        personName: personName
      }
    }
  });
};

export const unAssignPerson = async id => {
  return await apollo.mutate({
    mutation: MUTATION_UNASSIGN_PERSON,
    variables: {
      id: id
    }
  });
};

export const unAssignAllPrecictedByMedia = async mediaId => {
  return await apollo.mutate({
    mutation: MUTATION_UNASSIGN_PREDICTED_BY_MEDIA,
    variables: {
      mediaId
    }
  });
};


export const approveFace = async id => {
  return await apollo.mutate({
    mutation: MUTATION_APPROVE_FACE,
    variables: {
      id: id
    }
  });
};

export const approveAllByMedia = async mediaId => {
  return await apollo.mutate({
    mutation: MUTATION_APPROVE_ALL_BY_MEDIA,
    variables: {
      mediaId
    }
  });
};

export const deleteFace = async id => {
  return await apollo.mutate({
    mutation: MUTATION_DELETE_FACE,
    variables: {
      id: id
    }
  });
};

export const deleteUnassignedByMedia = async mediaId => {
  return await apollo.mutate({
    mutation: MUTATION_DELETE_UNASSIGNED_BY_MEDIA,
    variables: {
      mediaId
    }
  });
};

export const predictPerson = async faceId => {
  return await apollo.mutate({
    mutation: MUTATION_PREDICT_PERSON,
    variables: {
      input: {
        faceId: faceId
      }
    }
  });
};

export const predictPersonsByMedia = async mediaId => {
  return await apollo.mutate({
    mutation: MUTATION_PREDICT_PERSONS_BY_MEDIA,
    variables: {
      input: {
        mediaId
      }
    }
  });
};

export const buildPersonModel = async () => {
  return await apollo.mutate({
    mutation: MUTATION_BUILD_PERSON_MODEL,
    variables: {}
  });
};
