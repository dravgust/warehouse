import React, { useState, useEffect, useRef } from "react";
import { useQuery } from "react-query";
import SuiBox from "components/SuiBox";
import { Card, Icon, IconButton, Tooltip } from "@mui/material";
import SuiTypography from "components/SuiTypography";
import SuiButton from "components/SuiButton";
import ProductItem from "../product-item";
import Pagination from "@mui/material/Pagination";
import Stack from "@mui/material/Stack";
import FileUploadOutlinedIcon from "@mui/icons-material/FileUploadOutlined";
import { fetchProducts } from "utils/query-keys";
import { getProducts } from "services/warehouse-service";
import axios from "../../../../api";
import CircularProgress from "@mui/material/CircularProgress";

function ProductList({
  searchTerm,
  selectedItem,
  selectItem = () => {},
  resetToDefault = () => {},
  refresh,
}) {
  const [reload, updateReloadState] = useState(null);
  const forceUpdate = () => updateReloadState(Date.now());

  const [page, setPage] = useState(1);
  //const client = useClient();
  const pageSize = 3;
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchProducts, page, searchTerm, pageSize, refresh, reload], getProducts);

  useEffect(() => {
    if (isSuccess && response.items.length > 0) {
      if (selectedItem) {
        let e = response.items.find((item) => selectedItem.id === item.id);
        if (e) {
          selectItem(e);
        } else if (selectedItem.id) {
          selectItem(null);
        }
      }
    }
  }, [isSuccess]);

  const [progress, setProgress] = useState(0); // progress bar
  //const [file, setFile] = useState("");
  const fileInput = useRef();
  const handleFileChange = async (e) => {
    console.log(e);
    const file = e.target.files[0];
    //const file = fileInput.files[0];
    //setFile(file);
    await uploadFile(file);
  };

  const uploadFile = async (file) => {
    const formData = new FormData();
    formData.append("file", file, file.name);
    try {
      await axios.post(`products/file/upload`, formData, {
        onUploadProgress: (ProgressEvent) => {
          let progress = Math.round((ProgressEvent.loaded / ProgressEvent.total) * 100);
          setProgress(progress);
        },
      });
      //await client(`products/file/upload`, { formData });
      forceUpdate();
    } catch (err) {
      console.log("upload-file", err);
    }
    setProgress(0);
  };

  return (
    <Card id="product-list">
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" pt={3} px={3}>
        <SuiBox pt={0} px={2}>
          <SuiTypography variant="h6" fontWeight="medium">
            Products
          </SuiTypography>
          <SuiBox display="flex" alignItems="center" lineHeight={0}>
            <Icon
              sx={{
                fontWeight: "bold",
                color: ({ palette: { info } }) => info.main,
                mt: -0.5,
              }}
            >
              done
            </Icon>
            <SuiTypography variant="button" fontWeight="regular" color="text">
              &nbsp;<strong>{response && response.totalItems}</strong> items
            </SuiTypography>
          </SuiBox>
        </SuiBox>

        <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
          <SuiButton variant="contained" color="primary" onClick={resetToDefault}>
            <Icon>add</Icon>
            &nbsp;new
          </SuiButton>
          {/* <IconButton
            size="medium"
            color="primary"
            aria-label="add new product"
            onClick={resetToDefault}
          >
            <AddIcon fontSize="medium" />
          </IconButton>*/}
          <IconButton color="inherit" component="label">
            <input
              hidden
              accept=".xls, .xlsx"
              type="file"
              ref={fileInput}
              onChange={handleFileChange}
            />
            {progress ? (
              <SuiBox sx={{ position: "relative", display: "inline-flex" }}>
                <CircularProgress variant="determinate" />
                <SuiBox
                  sx={{
                    top: 0,
                    left: 0,
                    bottom: 0,
                    right: 0,
                    position: "absolute",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                  }}
                >
                  <SuiTypography variant="caption" component="div" color="text.secondary">
                    {`${Math.round(progress)}%`}
                  </SuiTypography>
                </SuiBox>
              </SuiBox>
            ) : (
              <FileUploadOutlinedIcon fontSize="medium" />
            )}
          </IconButton>
          <IconButton color="inherit" onClick={forceUpdate}>
            <Tooltip title="Reload">
              <Icon fontSize="medium">sync</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox pt={1} pb={2} px={2}>
        <SuiBox component="ul" display="flex" flexDirection="column">
          {isSuccess &&
            response.items.map((item) => (
              <ProductItem
                isSelected={selectedItem && selectedItem.id === item.id}
                key={item.id}
                item={item}
                onClick={() => {
                  selectItem(item);
                }}
              />
            ))}
          {isLoading && <SuiTypography color="secondary">Loading..</SuiTypography>}
          {error && <SuiTypography color="error">Error occurred!</SuiTypography>}
        </SuiBox>
        {isSuccess && page && (
          <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
            <Stack direction="row" spacing={2}>
              <Pagination
                count={response.totalPages}
                page={page}
                onChange={(event, value) => setPage(value)}
              />
            </Stack>
          </SuiBox>
        )}
      </SuiBox>
    </Card>
  );
}

export default ProductList;
