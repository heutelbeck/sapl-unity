using csharp.sapl.constraint;
using csharp.sapl.pdp.api;
using System;

namespace csharp.sapl.pep
{
    public class PolicyEnforcementPoint
	{
		private readonly IPolicyDecisionPoint pdp;
		private readonly ConstraintEnforcementService constraintEnforcementService;

		public ConstraintEnforcementService ConstraintEnforcementService { get => constraintEnforcementService; }
		public IPolicyDecisionPoint Pdp { get => pdp; }

		public PolicyEnforcementPoint(IPolicyDecisionPoint pdp, ConstraintEnforcementService service)
		{
			this.pdp = pdp;
			this.constraintEnforcementService = service;
		}

		//public IObservable<Boolean> IsPermitted(AuthorizationSubscription authzSubscription)
		//{
		//	IObservable <AuthorizationDecision> decision = pdp.Decide(authzSubscription);
		//	var authorizationDecision = decision.SelectMany(this.EnforceDecision);
		//	var temp = authorizationDecision;

		//	return pdp.Decide(authzSubscription).Next<AuthorizationDecision>().defaultIfEmpty(AuthorizationDecision.DENY)
		//			.flatMap(this::enforceDecision);
		//}

		//public IObservable<Boolean> IsPermitted(AuthorizationSubscription authzSubscription)
		//{
		//	IObservable<Boolean> returnValue = ;
		//		return pdp.Decide(authzSubscription);
		//	//	//				.next().defaultIfEmpty(AuthorizationDecision.DENY)
		//		//					.flatMap(this::enforceDecision);
		//	 return returnValue;
		//}

		private IObservable<Boolean> EnforceDecision(AuthorizationDecision authzDecision)
		{
			IObservable<Boolean> decision = null;

			//	if (authzDecision.getResource().isPresent())
			//		return Mono.just(Boolean.FALSE);

			//	return constraintEnforcementService
			//			.enforceConstraintsOfDecisionOnResourceAccessPoint(authzDecision, Flux.empty(), Object.class)
			//		.then(Mono.just(authzDecision.getDecision() == Decision.PERMIT))
			//		.onErrorReturn(AccessDeniedException.class, Boolean.FALSE);
			return decision;
		}
	}
}

//@Service
//@RequiredArgsConstructor
//public class PolicyEnforcementPoint
//{

//	private final PolicyDecisionPoint pdp;

//	private final ConstraintEnforcementService constraintEnforcementService;

//	public Mono<Boolean> isPermitted(AuthorizationSubscription authzSubscription)
//	{
//		return pdp.decide(authzSubscription).next().defaultIfEmpty(AuthorizationDecision.DENY)
//				.flatMap(this::enforceDecision);
//	}

//	private Mono<Boolean> enforceDecision(AuthorizationDecision authzDecision)
//	{
//		if (authzDecision.getResource().isPresent())
//			return Mono.just(Boolean.FALSE);

//		return constraintEnforcementService
//				.enforceConstraintsOfDecisionOnResourceAccessPoint(authzDecision, Flux.empty(), Object.class)
//				.then(Mono.just(authzDecision.getDecision() == Decision.PERMIT))
//				.onErrorReturn(AccessDeniedException.class, Boolean.FALSE);
//	}

//}