import DashboardLayout from "../../examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "../../examples/Navbars/DashboardNavbar";
import Footer from "../../examples/Footer";
import SuiBox from "../../components/SuiBox";
import Grid from "@mui/material/Grid";
import {Zoom} from "@mui/material";
import {useState} from "react";
import BeaconList from "./components/beacon-list";
import SelectedBeacon from "./components/selected-beacon";

const Beacons = () => {
    const [searchTerm, setSearchTerm] = useState("");
    const onSearch = (value) => setSearchTerm(value);

    const [selectedItem, selectItem] = useState(null);

    const [refresh, updateRefreshState] = useState(0);
    const forceUpdate = () => updateRefreshState(Date.now());

    const resetToNull = () => selectItem(null);
    const resetToDefault = () => selectItem({
        macAddress:'',
        name:''
    });
    const onSelectItem =(item, key) => {
        selectItem({...item, key: key});
    };
    function handleDelete() {
        resetToNull();
        forceUpdate();
    }

    const handleSave = () => forceUpdate();

    return(
        <DashboardLayout>
            <DashboardNavbar onSearch={onSearch} />
            <SuiBox>
                <SuiBox mb={3} py={3}>
                    <Grid container spacing={3}>
                        <Grid item xs={12} lg={selectedItem ? 5 : 12}>
                            <BeaconList
                                searchTerm={searchTerm}
                                selectedItem={selectedItem}
                                onRowSelect={onSelectItem}
                                onAdd={resetToDefault}
                                refresh={refresh}/>
                        </Grid>
                        <Zoom in={Boolean(selectedItem)}>
                            <Grid item xs={12} lg={7}>
                                {selectedItem && (
                                    <SelectedBeacon
                                        item={selectedItem}
                                        onSave={handleSave}
                                        onDelete={handleDelete}
                                        onClose={resetToNull}
                                        products={[]}/>
                                )}
                            </Grid>
                        </Zoom>
                    </Grid>
                </SuiBox>
            </SuiBox>
            <Footer/>
        </DashboardLayout>
    )
}

export default Beacons;