
module.exports = {
    projects: {
        app: {
            schema: "http://localhost:5000/graphql/",
            documents: ["./src/**/graphql/**/*.gql"],
            extensions: {
                endpoints: {
                    default: {
                        url: "http://localhost:5000/graphql/",
                    },
                },
            }
        },
    },
}