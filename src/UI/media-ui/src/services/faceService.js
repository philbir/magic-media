import apollo from '../apollo'
import MUTATION_ASSIGN_PERSON from "../graphql/AssignPersonByHuman.gql";
import MUTATION_UNASSIGN_PERSON from "../graphql/UnassignPersonFromFace.gql";
import MUTATION_DELETE_FACE from "../graphql/DeleteFace.gql";

 export const assignPerson = async (faceId, personName) => {
    return await apollo.mutate({
        mutation: MUTATION_ASSIGN_PERSON,
        variables: {
          input: {
            faceId: faceId,
            personName: personName,
          },
        },
      });
 };

 export const unAssignPerson = async (id) => {
  return await apollo.mutate({
      mutation: MUTATION_UNASSIGN_PERSON,
      variables: {
        id: id
      },
    });
};

export const deleteFace = async (id) => {
  return await apollo.mutate({
      mutation: MUTATION_DELETE_FACE,
      variables: {
        id: id
      },
    });
};