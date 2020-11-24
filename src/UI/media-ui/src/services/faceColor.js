export const getFaceColor = face => {
  if (face.person) {
    if (face.recognitionType === "COMPUTER") {
      if (face.state === "VALIDATED") {
        return "#307efc";
      } else {
        return "#f05656";
      }
    } else return "#036d05";
  } else {
    return "#fce300";
  }
};
