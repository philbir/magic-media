export const getFaceColor = (face) => {
    if (face.person) {
      if (face.recognitionType === "COMPUTER") {
        if (face.state === "VALIDATED") {
          return "#00368e";
        } else {
          return "#6346e2";
        }
      } else return "#036d05";
    } else {
      return "#fce300";
    }
  };
  