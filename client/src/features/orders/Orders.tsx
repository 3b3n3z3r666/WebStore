import { Button, Paper, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import agent from "../../app/api/agent";
import { useEffect, useState } from "react";
import LoadingComponent from "../../app/layout/LoadingComponent";
import { Order, OrderItem } from "../../app/models/order";
import { currencyFormat } from "../../app/util/util";
import { number } from "yup";
import BasketTable from "../basket/BasketTable";
import { BasketItem } from "../../app/models/basket";
import OrderDetails from "./OrderDetails";

export default function Orders() {

    const [orders, setOrders] = useState<Order[] | null>(null);
    const [loading, setLoading] = useState(true);

    const [selectedOrderNumber, setSelectedOrderNumber] = useState(0);

    useEffect(() => {
        agent.Orders.list()
            .then(orders => setOrders(orders))
            .catch(error => console.log(error))
            .finally(() => setLoading(false))
    }, [])

    const handleSelectOrder = (orderNumber: number) => {
        setSelectedOrderNumber(orderNumber)
    }

    if (loading) return (< LoadingComponent message="Loading Orders..." />);


    if (selectedOrderNumber > 0)
        return (
            <OrderDetails
                order={orders?.find(order => order.id === selectedOrderNumber)!}
                setSelectedOrderNumber={setSelectedOrderNumber}
            />
        )
    return (
        <TableContainer component={Paper}>
            <Table sx={{ minWidth: 650 }} aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell>Order number </TableCell>
                        <TableCell align="right"> Total</TableCell>
                        <TableCell align="right">Order Date</TableCell>
                        <TableCell align="right">Order Status</TableCell>
                        <TableCell align="right"></TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {orders?.map((order) => (
                        <TableRow
                            key={order.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell component="th" scope="row">
                                {order.id}
                            </TableCell>
                            <TableCell align="right">{currencyFormat(order.total)}</TableCell>
                            <TableCell align="right">{order.orderDate.split("T")[0]}</TableCell>
                            <TableCell align="right">{order.orderStatus}</TableCell>
                            <TableCell align="right">
                                <Button onClick={() => handleSelectOrder(order.id)}>View </Button>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>

    )


}