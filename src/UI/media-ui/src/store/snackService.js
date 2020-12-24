
export const addSnack = (dispatch, text, type = "SUCCESS") => {
    dispatch(
        "snackbar/addSnack",
        { text, type },
        { root: true }
    );
}