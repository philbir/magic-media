export const mediaOperationTypeMap = {
  0: {
    title: "Move media",
    verb: "moved",
    completedText: "Move completed.",
    removeFromList: true,
  },
  1: {
    title: "Recycle media",
    verb: "recycled",
    completedText: "Recycle completed.",
    removeFromList: true,
  },
  2: {
    title: "Update metadata",
    verb: "metadata updated",
    completedText: "Metadata update completed.",
    removeFromList: false,
  },
  3: {
    verb: "faces rescanned",
    completedText: "Face rescan completed.",
    removeFromList: false,
  },
  4: {
    title: "Delete media",
    verb: "deleted",
    completedText: "Delete completed.",
    removeFromList: true,
  },
  5: {
    title: "Export media",
    verb: "exported",
    completedText: "Export completed.",
    removeFromList: false,
  }
};
