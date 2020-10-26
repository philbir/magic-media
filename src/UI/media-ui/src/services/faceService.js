import apollo from '../apollo'
import MUTATION_ASSIGN_PERSON from "../graphql/AssignPersonByHuman.gql";

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
