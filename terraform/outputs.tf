output "cluster_name" {
  value = module.eks.cluster_id
}

output "aws_region" {
  value = var.aws_region
}
