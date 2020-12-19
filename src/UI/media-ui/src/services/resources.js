export const resources = {
    navMenu: [
        {
            text: "Media",
            icon: "mdi-image",
            route: "Home",
        },
        {
            text: "Face",
            icon: "mdi-face-recognition",
            route: "Faces",
        },
        {
            text: "Persons",
            icon: "mdi-account-details",
            route: "Persons",
        },
        {
            text: "Albums",
            icon: "mdi-image-album",
            route: "Albums",
        },
        {
            text: "Map",
            icon: "mdi-map-search-outline",
            route: "Map",
        },
        {
            text: "Users",
            icon: "mdi-account",
            route: "Users",
            auth: "USER_MANAGE"
        },
        {
            text: "Settings",
            icon: "mdi-tune-variant",
            route: "Settings",
            auth: "GENERAL_SETTINGS"
        },
    ]
}

export const getAuthorized = (list, permissions) => {
    return list.filter(x => {
        if (x.auth) {

            return permissions.includes(x.auth);
        }
        return true;
    });
}

