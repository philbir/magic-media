export const parseAddress = (place) => {

    const parsed = {
        name: place.formatted_address
    };
    for (let i = 0; i < place.address_components.length; i++) {
        const item = place.address_components[i];

        if (item.types.some(r =>
            ["establishment", "point_of_interest", "transit_station", "landmark"]
                .indexOf(r) >= 0)) {
            parsed.place = item.long_name;
        }
        else if (item.types.includes("route")) {
            parsed.street = item.long_name;
        }
        else if (item.types.includes("street_number")) {
            parsed.streetNumber = item.long_name;
        }
        else if (item.types.includes("postal_code")) {
            parsed.zip = item.long_name;
        }
        else if (item.types.includes("locality")) {
            parsed.city = item.long_name;
        }
        else if (item.types.includes("administrative_area_level_1")) {
            parsed.district1 = item.long_name;
        }
        else if (item.types.includes("administrative_area_level_2")) {
            parsed.district2 = item.long_name;
        }
        else if (item.types.includes("country")) {
            parsed.country = item.long_name;
            parsed.countryCode = item.short_name;
        }
    }

    return parsed;
};