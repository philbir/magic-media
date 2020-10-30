import apollo from '../apollo'
import QUERY_ALL_PERSONS from "../graphql/AllPersons.gql";

 export const getAllPersons = async () => {
    return await apollo.query({
     query: QUERY_ALL_PERSONS,
     variables: {
 
     }
   });
 };
