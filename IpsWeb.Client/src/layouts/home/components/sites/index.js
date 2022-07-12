import {Card, Icon} from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import Table from "examples/Tables/Table";
import * as auth from "auth-provider";
import {client} from "utils/api-client";
import {useQuery} from "react-query";

function SiteWithProduct({onSelect = () => {}, selectedItem}) {

    async function fetchSites(productId) {
        const token = await auth.getToken();
        const res = await client(`assets/sites?productId=${productId}`, {token});
        return res;
    }

    const {isLoading, isSuccess, data, error} = useQuery(
        ["site-list-product", selectedItem],
        () => fetchSites(selectedItem.product.id),
        {
            keepPreviousData: false,
            refetchOnWindowFocus: false,
            enabled: Boolean(selectedItem && selectedItem.product && selectedItem.product.id),
        }
    );

    return (
        <Card>
            <SuiBox display="flex" justifyContent="space-between" alignItems="center"  pt={3} px={2}>
                <SuiBox>
                    <SuiTypography variant="h6">Product</SuiTypography>
                </SuiBox>
            </SuiBox>
            <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={2}>
                <SuiTypography variant="h5" fontWeight="bold" gutterBottom color="primary">
                    {selectedItem.product && selectedItem.product.name}
                </SuiTypography>
            </SuiBox>
            <SuiBox pt={3} pb={2} px={2}>
                <SuiTypography variant="caption" color="text" fontWeight="bold" textTransform="uppercase">
                    Sites
                </SuiTypography>
            </SuiBox>
            <SuiBox
                sx={{
                    "& .MuiTableRow-root:not(:last-child)": {
                        "& td": {
                            borderBottom: ({borders: {borderWidth, borderColor}}) =>
                                `${borderWidth[1]} solid ${borderColor}`,
                        },
                    },
                }}
            >
                {isSuccess && (
                    <Table
                        columns={[
                            {name: "name", align: "left"},
                            {name: "", align: "center"},
                        ]}
                        rows={
                            data.map((item) => ({
                                key: item.id,
                                item,
                                name: (
                                    <SuiTypography variant="button" fontWeight="medium">
                                        {item.name}
                                    </SuiTypography>
                                ),
                                "": (
                                    <SuiTypography variant="caption" color="secondary">

                                    </SuiTypography>
                                ),
                            }))
                        }
                        onSelect={onSelect}
                        selectedKey={selectedItem ? selectedItem.siteId : ""}
                    />
                )}
                {isLoading && (
                    <SuiTypography px={2} color="secondary">
                        Loading..
                    </SuiTypography>
                )}
                {error && (
                    <SuiTypography px={2} color="error">
                        Error occurred!
                    </SuiTypography>
                )}
            </SuiBox>
        </Card>
    );
}

export default SiteWithProduct;