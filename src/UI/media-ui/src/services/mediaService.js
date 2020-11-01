import apollo from '../apollo'
import QUERY_SEARCH from "../graphql/SearchMedia.gql";
import QUERY_GETBYID from "../graphql/GetMediaDetails.gql";

 export const  searchMedia = async (request) => {
    return await apollo.query({
     query: QUERY_SEARCH,
     variables: {
      request
     }
   });
 };

 export const  getById = async (id) => {
  return await apollo.query({
    query: QUERY_GETBYID,
    variables: {
      id: id,
    },
  });
};