from python_graphql_client import GraphqlClient
import os

client = GraphqlClient(
    endpoint=os.environ["MEDIA_API_URL"])

query = """
        query searchFaces {
            searchFaces(request: {}) {
                id
                personId
                encoding
            }
        }
"""
data = client.execute(query=query)
faces = data["data"]["searchFaces"]