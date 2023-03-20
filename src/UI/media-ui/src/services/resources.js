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
            auth: "USER_VIEW"
        },
        {
            text: "Audit log",
            icon: "mdi-monitor-eye",
            route: "AuditLog",
            auth: "AUDITLOG_VIEW"
        },
        {
            text: "Similar",
            icon: "mdi-compare",
            route: "Similar",
            auth: "USER_VIEW"
        },

        {
            text: "Settings",
            icon: "mdi-tune-variant",
            route: "Settings",
            auth: "GENERAL_SETTINGS"
        },
    ],
    mediaActions: [
        { text: "Add to album", action: "ADD_TO_ALBUM", icon: "mdi-plus" },
        { text: "Move", action: "MOVE", icon: "mdi-file-move-outline" },
        { text: "Edit", action: "EDIT", icon: "mdi-pencil" },
        { text: "Recycle", action: "RECYCLE", icon: "mdi-recycle" },
        { text: "ReScan faces", action: "RESCAN_FACE", icon: "mdi-face-recognition" },
        {
            text: "Delete",
            action: "DELETE",
            icon: "mdi-delete",
            auth: "MEDIA_DESTROY",
        },
        {
            text: "Share",
            action: "SHARE",
            icon: "mdi-share-variant-outline",
        },
        {
            text: "Export",
            action: "EXPORT",
            icon: "mdi-export",
        },
    ],
}

export const getAuthorized = (list, permissions) => {
    return list.filter(x => {
        if (x.auth) {
            return permissions.includes(x.auth);
        }
        return true;
    });
}

