import apollo from "../apollo";
import QUERY_ALL_PERSONS from "../graphql/Person/AllPersons.gql";
import QUERY_ALL_GROUPS from "../graphql/Person/AllGroups.gql";
import QUERY_SEARCH from "../graphql/Person/search.gql";
import MUTATION_UPDATE_PERSON from "../graphql/Person/UpdatePersonRequest.gql";
import MUTATION_CREATE_GROUP from "../graphql/Person/CreateGroup.gql";

export const getAllPersons = async () => {
  return await apollo.query({
    query: QUERY_ALL_PERSONS,
    variables: {}
  });
};

export const getAllGroups = async () => {
  return await apollo.query({
    query: QUERY_ALL_GROUPS,
    variables: {}
  });
};

export const search = async (input) => {
  return await apollo.query({
    query: QUERY_SEARCH,
    variables: { input }
  });
};

export const updatePerson = async input => {
  return await apollo.mutate({
    mutation: MUTATION_UPDATE_PERSON,
    variables: {
      input: {
        id: input.id,
        name: input.name,
        dateOfBirth: input.dateOfBirth,
        groups: input.groups,
        newGroups: input.newGroups
      }
    }
  });
};

export const createGroup = async name => {
  return await apollo.mutate({
    mutation: MUTATION_CREATE_GROUP,
    variables: {
      name: name
    }
  });
};
