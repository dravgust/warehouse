import {useState} from "react";

// @mui material components
import Grid from "@mui/material/Grid";
import Icon from "@mui/material/Icon";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";

// Soft UI Dashboard React examples
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";


import ProductsTreeView from "./components/products";
import PositionEvents from "./components/position-events";
import Assets from "./components/beacons";
import DefaultInfoCard from "examples/Cards/InfoCards/DefaultInfoCard";
import {Stack, Zoom} from "@mui/material";
import Beacon from "./components/beacon";
import Sites from "./components/sites";

function Dashboard() {
    const [searchTerm, setSearchTerm] = useState("");
    const onSearch = (value) => setSearchTerm(value);

    const [selectedProduct, setSelectProduct] = useState(null);
    const [selectedBeacon, setSelectBeacon] = useState(null);
    const [selectedList, setSelectList] = useState('product');

    const onListSelect = (listName) => {
        setSelectList(listName);
        setSelectProduct(null);
        setSelectBeacon(null);
    }

    return (
        <DashboardLayout>
            <DashboardNavbar onSearch={onSearch}/>
            <SuiBox mb={3} py={3}>
                <Grid container spacing={3}>
                    <Grid item xs={12} md={6} lg={4} mb={3}>
                        <Stack spacing={3}>
                            {selectedList === 'product' && (
                                <ProductsTreeView
                                    selectedProduct={selectedProduct}
                                    onProductSelect={setSelectProduct}
                                    selectedBeacon={selectedBeacon}
                                    onBeaconSelect={setSelectBeacon}
                                    onListSelect={onListSelect}
                                />
                            )}
                            {selectedList === 'beacon' && (
                                <Assets
                                    onListSelect={onListSelect}
                                    onRowSelect={(row) => setSelectBeacon({macAddress: row.macAddress, name: row.site.name})}
                                    searchTerm={searchTerm}
                                    selectedItem={selectedBeacon}
                                />
                            )}
                            {selectedList === 'site' && (
                                <Sites />
                            )}
                        </Stack>
                    </Grid>

                    {Boolean(selectedBeacon) && (
                        <Grid item xs={12} md={6} lg={4} mb={3}>
                            <Grid container spacing={3}>
                                <Grid item xs={12}>
                                    <Beacon macAddress={selectedBeacon.macAddress} name={selectedBeacon.name}/>
                                </Grid>
                                <Grid item xs={12} md={6}>
                                    <DefaultInfoCard
                                        icon="thermostat"
                                        title="Temperature"
                                        description="Ambient Temperature"
                                        value="32C&deg;"
                                    />
                                </Grid>
                                <Grid item xs={12} md={6}>
                                    <DefaultInfoCard
                                        icon="waves"
                                        title="Humidity"
                                        description="Absolute humidity"
                                        value="45%"
                                    />
                                </Grid>

                            </Grid>
                        </Grid>
                    )}

                    <Grid item xs={12} md={6} lg={4}>
                        <PositionEvents searchTerm={selectedBeacon ? selectedBeacon.macAddress : ''}/>
                    </Grid>
                </Grid>
            </SuiBox>
            <Footer/>
        </DashboardLayout>
    );
}

export default Dashboard;
