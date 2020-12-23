export const excuteGraphQL = async (call, dispatch) => {

    try {
        const response = await call();

        if (response.errors) {
            return {
                success: false,
                data: response.data
            }
        }
        return {
            success: true,
            data: response.data
        }
    }
    catch {
        dispatch(
            "snackbar/addSnack",
            { text: "An API error has accoured.", type: "ERROR" },
            { root: true }
        );

        return {
            success: false,
        }
    }
} 