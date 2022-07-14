import { useState } from "react";
// @mui material components
import Card from "@mui/material/Card";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

/* eslint-disable react/prop-types */
import SuiAvatar from "components/SuiAvatar";
import SuiBadge from "components/SuiBadge";

// Soft UI Dashboard React examples
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import Table from "examples/Tables/Table";

import { useQuery } from "react-query";
import { client } from "utils/api-client";
import * as auth from "auth-provider";
import { format } from "date-fns";

// Images
import userIcon from "assets/images/user.png";

function ProviderName(providerId) {
  switch (providerId) {
    case 1:
      return "Electra";
    case 2:
      return "Dolav";
    case 3:
      return "Meitav";
    case 4:
      return "Tel-Aviv University";
    case 1000:
      return "Vayosoft";
    default:
      return "Unknown";
  }
}

function User({ image, name, email }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <SuiAvatar src={image} alt={name} size="sm" variant="rounded" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium">
          {name}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {email || ""}
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}

function Function({ job, org }) {
  return (
    <SuiBox display="flex" flexDirection="column">
      <SuiTypography variant="caption" fontWeight="medium" color="text">
        {job}
      </SuiTypography>
      <SuiTypography variant="caption" color="secondary">
        {org}
      </SuiTypography>
    </SuiBox>
  );
}

function Users() {
  const [page, setPage] = useState(1);
  const fetchItems = async (page) => {
    const token = await auth.getToken();
    const res = await client(`users?page=${page}&take=9`, { token });
    return res;
  };
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery(["list-users", page], () => fetchItems(page), {
    keepPreviousData: false,
    refetchOnWindowFocus: false,
  });

  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3}>
        <SuiBox mb={3}>
          <Card>
            <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
              <SuiTypography variant="h6">Users</SuiTypography>
            </SuiBox>
            <SuiBox
              sx={{
                "& .MuiTableRow-root:not(:last-child)": {
                  "& td": {
                    borderBottom: ({ borders: { borderWidth, borderColor } }) =>
                      `${borderWidth[1]} solid ${borderColor}`,
                  },
                },
              }}
            >
              {isSuccess && (
                <Table
                  columns={[
                    { name: "user", align: "left" },
                    { name: "function", align: "left" },
                    { name: "status", align: "center" },
                    { name: "registed", align: "center" },
                    { name: "action", align: "center" },
                  ]}
                  rows={
                    isSuccess &&
                    response.data.map((item) => ({
                      user: <User image={userIcon} name={item.username} email={item.email} />,
                      function: <Function job={item.kind} org={ProviderName(item.providerId)} />,
                      status: (
                        <SuiBadge
                          variant="gradient"
                          badgeContent="n/a"
                          color="dark"
                          size="xs"
                          container
                        />
                      ),
                      registed: (
                        <SuiTypography variant="caption" color="secondary" fontWeight="medium">
                          {format(new Date(item.registrationDate), "dd/MM/yyy HH:mm:ss")}
                        </SuiTypography>
                      ),
                      action: (
                        <SuiTypography
                          component="a"
                          href="#"
                          variant="caption"
                          color="secondary"
                          fontWeight="medium"
                        >
                          ...
                        </SuiTypography>
                      ),
                    }))
                  }
                  page={page}
                  totalPages={response.totalPages}
                  onPageChange={(event, value) => setPage(value)}
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
        </SuiBox>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
}

export default Users;
