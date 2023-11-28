import { Elements } from "@stripe/react-stripe-js";
import CheckoutPage from "./CheckoutPage";
import { loadStripe } from "@stripe/stripe-js";
import { useAppDispatch } from "../../app/store/configureStore";
import agent from "../../app/api/agent";
import { useEffect, useState } from "react";
import { setBasket } from "../basket/basketSlice";
import LoadingComponent from "../../app/layout/LoadingComponent";


const stripePromise = loadStripe("pk_test_51ODBo1FwlogWasXXCKia1zm8yAdXh8Qu8mxhfFpowbKs3nIpnbUHxSb5qeMIRtRgRaxpAlDKGPJBENx9wjUUBOiU00gv7xToPC");

export default function CheckoutWrapper(){

    const dispatch = useAppDispatch();
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        agent.Payments.createPaymentIntent()
        .then(basket => dispatch(setBasket(basket)))
        .then(error => console.log(error))
        .finally(() => setLoading(false))
    }, [dispatch])

    if (loading) return <LoadingComponent message="Loading checkout..."/>

    return (
        <Elements stripe={stripePromise}>
            <CheckoutPage />
        </Elements>
    )
}